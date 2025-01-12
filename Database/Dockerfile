FROM postgres:14

# Set environment variables (you can also pass them dynamically with a .env file or through docker-compose)
ENV POSTGRES_USER=${POSTGRES_USER}
ENV POSTGRES_PASSWORD=${POSTGRES_PASSWORD}
ENV POSTGRES_DB=${POSTGRES_DB}


COPY postgresql.conf /etc/postgresql/postgresql.conf
COPY pg_hba.conf /etc/postgresql/pg_hba.conf
# Optional: Add a custom initialization SQL script
# If you have a custom database initialization script, you can copy it into the container
COPY Main.sql /docker-entrypoint-initdb.d/01-Main.sql

# Volumes for persistence (handled in docker-compose for data storage)
VOLUME /var/lib/postgresql/data

# Expose the Postgres default port
EXPOSE 5432
