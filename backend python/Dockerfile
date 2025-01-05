# Use an official Python runtime as the base image
FROM python:3.12-slim

# Set environment variables to disable bytecode generation and Poetry virtualenv creation
ENV PYTHONDONTWRITEBYTECODE=1 \
    PYTHONUNBUFFERED=1 \
    POETRY_VIRTUALENVS_CREATE=false

# Install system dependencies
RUN apt-get update && apt-get install -y --no-install-recommends \
    curl \
    && rm -rf /var/lib/apt/lists/*

# Install Poetry
RUN curl -sSL https://install.python-poetry.org | python3 -
ENV PATH="/root/.local/bin:$PATH"

# Set the working directory
WORKDIR /app

# Copy Poetry configuration files
COPY . .
COPY pyproject.toml poetry.lock ./

# Install dependencies using Poetry
RUN poetry update

# Copy the rest of the application code

# Expose the port your Flask app runs on
# Define the command to run the application
CMD ["python", "app.py"]
