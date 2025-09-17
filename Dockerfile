FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS builder

WORKDIR /app


# Set DataDog Environment Variables
ENV CORECLR_ENABLE_PROFILING=1
ENV CORECLR_PROFILER={846F5F1C-F9AE-4B07-969E-05C26BC060D8}
ENV DD_DOTNET_TRACER_HOME=/app/datadog
# for alpine
ENV CORECLR_PROFILER_PATH=/app/datadog/linux-musl-x64/Datadog.Trace.ClrProfiler.Native.so
ENV LD_PRELOAD=/app/datadog/linux-musl-x64/Datadog.Linux.ApiWrapper.x64.so
# other DD variables
ENV DD_INTEGRATIONS=/app/datadog/integrations.json

# env variable for build to know which project is the main one
ARG MAIN_PROJECT_NAME='SchedulerJob'

# copy code over and remove any environment specific appsettings files, config will be injected by Helm
#ADD . .
#RUN rm -f $MAIN_PROJECT_NAME/appsettings.*.json

RUN echo "Current directory: $(pwd)"
RUN echo "listing files in directory: "  
RUN ls -a

# restore each project (will use sln to know all the local projects)
COPY . .

RUN echo "Current directory: $(pwd)"
RUN echo "listing files in directory after copy: "  
RUN ls -a
RUN dotnet restore "SchedulerJob/SchedulerJob.csproj"
# this will add the restore to layers separate from build/publish


# Publish the main project
RUN dotnet publish $MAIN_PROJECT_NAME --output /app/published -c Release

RUN echo $MAIN_PROJECT_NAME > /app/published/__assemblyname

# Main Container Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine

LABEL repo='https://git.ascendlearning.com/ascend/examfx/services/scheduler-job' \
	company='Ascend Learning' \
	businessunit='examfx' \
	team='examfx-team' \
	maintainer='david.dickerson@ascendlearning.com'



# update alpine packages
RUN apk update && \
    apk upgrade --available && \
    apk add --no-cache libcrypto3 libssl3 icu-libs icu-data-en

RUN apk add icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Set default directory
WORKDIR /app

# Set DataDog Environment Variables
ENV CORECLR_ENABLE_PROFILING=1
ENV CORECLR_PROFILER={846F5F1C-F9AE-4B07-969E-05C26BC060D8}
ENV DD_DOTNET_TRACER_HOME=/app/datadog
# for alpine
ENV CORECLR_PROFILER_PATH=/app/datadog/linux-musl-x64/Datadog.Trace.ClrProfiler.Native.so
ENV LD_PRELOAD=/app/datadog/linux-musl-x64/Datadog.Linux.ApiWrapper.x64.so
# other DD variables
ENV DD_PROFILING_ENABLED=1
ENV DD_INTEGRATIONS=/app/datadog/integrations.json
ENV DD_LOGS_INJECTION=true
ENV DD_RUNTIME_METRICS_ENABLED=true



# Copy over published binaries from build container to runtime image build
COPY --from=builder /app/published/ .
ADD entrypoint.sh/ /entrypoint.sh

# Create the new user and group
RUN addgroup -g 1000 -S dotnet && \
    adduser -u 1000 -S dotnet -G dotnet && \
    # Change the executable to be owned by the new user and group
    chown -R dotnet:dotnet /app /entrypoint.sh && \
    # Only read+write+execute permissions.
    # chmod -R 0500 /app && \
    chmod -R 0755 /app && \
    # Setup DataDog Log Dir
    mkdir -p /var/log/datadog && \
    chown -R dotnet:dotnet /var/log/datadog && \
    chmod -R ug+w /var/log/datadog && \
    chmod +x /entrypoint.sh

# Switch container context to run as user (non-root).
# FYI: if running in Kubernetes then use the number ID and not the string name of the new user
USER 1000

EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080
# Remove the appsettings.json file if it exists
#RUN if [ -f SchedulerJob/appsettings.json ]; then rm SchedulerJob/appsettings.json; fi


# Copy the build appsettings.json to appsettings.json
RUN rm appsettings.json
RUN cp appsettings.build.json appsettings.json

# Start application
# Will check if vault.env file exists and if does will inject those as env vars before starting the dotnet application 
RUN echo "Current directory in the end: $(pwd)"
RUN echo "listing files in directory: "  
RUN ls -a

RUN echo "listing files in SchedulerJob directory: "
RUN ls -a SchedulerJob
ENTRYPOINT ["sh", "-c", "/entrypoint.sh $(cat /app/__assemblyname).dll"]
