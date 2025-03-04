# Use the official .NET 8.0 devcontainer image as the base
FROM mcr.microsoft.com/devcontainers/dotnet:8.0

# Install required packages: curl and python3
RUN apt-get update && \
    apt-get install -y --no-install-recommends curl python3 && \
    rm -rf /var/lib/apt/lists/*

# Install Poetry using the official installer
RUN curl -sSL https://install.python-poetry.org | python3 -

# Add Poetry to the PATH; by default Poetry is installed in ~/.local/bin
ENV PATH="/root/.local/bin:${PATH}"

# Set the default shell to bash for ease of use
ENV SHELL=/bin/bash
