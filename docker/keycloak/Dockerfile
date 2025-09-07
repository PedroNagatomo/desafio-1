FROM quay.io/keycloak/keycloak:23.0

# Copy realm configuration
COPY import/ /opt/keycloak/data/import/

# Set environment variables
ENV KEYCLOAK_ADMIN=admin
ENV KEYCLOAK_ADMIN_PASSWORD=admin123
ENV KC_DB=postgres
ENV KC_HOSTNAME_STRICT=false
ENV KC_HOSTNAME_STRICT_HTTPS=false
ENV KC_HTTP_ENABLED=true
ENV KC_HOSTNAME_STRICT_BACKCHANNEL=false
ENV KC_HEALTH_ENABLED=true
ENV KC_METRICS_ENABLED=true

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:8080/health/ready || exit 1

# Install curl for health checks
USER root
RUN microdnf install -y curl && microdnf clean all
USER 1000

# Expose port
EXPOSE 8080

# Start Keycloak with import
ENTRYPOINT ["/opt/keycloak/bin/kc.sh", "start-dev", "--import-realm"]