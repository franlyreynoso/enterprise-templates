# Observability Architecture

This document describes the observability stack architecture across the enterprise templates, including the differences between template approaches and port configurations.

## Overview

The enterprise templates provide distributed tracing, structured logging, and metrics collection through OpenTelemetry and Jaeger. There are two architectural approaches used across the templates:

### Backend Template: All-in-One Architecture

The backend template uses Jaeger's all-in-one image which includes both the OTLP collector and Jaeger UI in a single container.

```yaml
services:
  otel:
    image: jaegertracing/all-in-one:1.57
    ports:
      - "${OTEL_UI_PORT:-16686}:16686"      # Jaeger UI
      - "${OTLP_GRPC_PORT:-4317}:4317"     # OTLP gRPC receiver
      - "${OTLP_HTTP_PORT:-4318}:4318"     # OTLP HTTP receiver
```

**Flow:**
```
Application → otel:4317/4318 → Jaeger UI:16686
             (all in one container)
```

### Web/Fullstack Templates: Modular Architecture

The web template and fullstack orchestrator use a two-service approach with separate OpenTelemetry Collector and Jaeger containers.

```yaml
services:
  otel-collector:
    image: otel/opentelemetry-collector-contrib:latest
    ports:
      - "${OTEL_GRPC_PORT:-4317}:4317"    # OTLP gRPC receiver (exposed)
      - "${OTEL_HTTP_PORT:-4318}:4318"    # OTLP HTTP receiver (exposed)
    
  jaeger:
    image: jaegertracing/all-in-one:latest
    ports:
      - "${JAEGER_UI_PORT:-16686}:16686"  # Jaeger UI only (exposed)
    # Note: Jaeger listens on 4317 internally but does NOT expose it to host
```

**Flow:**
```
Application → otel-collector:4317/4318 (exposed) → jaeger:4317 (internal) → Jaeger UI:16686 (exposed)
```

## Port Allocation

### Development Environment Ports

| Service          | Backend | Web/Fullstack | Purpose                    |
|------------------|---------|---------------|----------------------------|
| OTLP gRPC        | 4317    | 4317          | OpenTelemetry gRPC         |
| OTLP HTTP        | 4318    | 4318          | OpenTelemetry HTTP         |
| Jaeger UI        | 16686   | 16686         | Web interface for traces   |
| Jaeger gRPC      | -       | 14250         | Legacy Jaeger protocol     |
| Jaeger Thrift    | -       | 14268         | Legacy Jaeger protocol     |
| Jaeger Zipkin    | -       | 9411          | Zipkin compatibility       |

### Staging Environment Ports

Staging uses offset ports to avoid conflicts when running multiple environments:

| Service          | Port  |
|------------------|-------|
| OTLP gRPC        | 4318  |
| OTLP HTTP        | 4319  |
| Jaeger UI        | 16687 |
| Jaeger gRPC      | 14251 |
| Jaeger Thrift    | 14269 |
| Jaeger Zipkin    | 9412  |

### Production Environment Ports

Production uses further offset ports:

| Service          | Port  |
|------------------|-------|
| OTLP gRPC        | 4319  |
| OTLP HTTP        | 4320  |
| Jaeger UI        | 16688 |
| Jaeger gRPC      | 14252 |
| Jaeger Thrift    | 14270 |
| Jaeger Zipkin    | 9413  |

## Configuration

### OpenTelemetry Collector Configuration

The web and fullstack templates use `otel-collector-config.yaml` to configure the collector:

```yaml
receivers:
  otlp:
    protocols:
      grpc:
        endpoint: 0.0.0.0:4317
      http:
        endpoint: 0.0.0.0:4318

exporters:
  otlp:
    endpoint: jaeger:4317  # Internal connection to Jaeger
    tls:
      insecure: true
```

### Application Configuration

Applications should connect to the OpenTelemetry Collector endpoint:

**Backend Template:**
```
OTEL_EXPORTER_OTLP_ENDPOINT=http://localhost:4317
```

**Web/Fullstack Templates:**
```
OpenTelemetry__Endpoint: http://otel-collector:4317  # From within Docker network
OTEL_EXPORTER_OTLP_ENDPOINT=http://localhost:4317   # From host machine
```

## Common Issues

### Port Conflicts

**Problem:** Error "Bind for 0.0.0.0:4317 failed: port is already allocated"

**Cause:** Multiple services trying to expose the same port to the host.

**Solution:** Ensure only ONE service per environment exposes ports 4317 and 4318:
- In backend template: The `otel` service exposes these ports
- In web/fullstack templates: The `otel-collector` service exposes these ports, NOT the `jaeger` service

**Incorrect Configuration (causes port conflict):**
```yaml
# DON'T DO THIS - jaeger should not expose OTLP ports
jaeger:
  ports:
    - "16686:16686"
    - "4317:4317"  # ❌ Conflicts with otel-collector
    - "4318:4318"  # ❌ Conflicts with otel-collector
```

**Correct Configuration:**
```yaml
# Jaeger only exposes UI port
jaeger:
  ports:
    - "16686:16686"  # ✅ UI only
```

### Internal vs External Ports

The Jaeger service in the modular architecture (web/fullstack templates) needs to:
- **Listen** on port 4317 internally (for otel-collector to send data)
- **NOT expose** port 4317 to the host (no port mapping)

This is handled automatically by the `jaegertracing/all-in-one` image when `COLLECTOR_OTLP_ENABLED: true` is set.

## Testing

To verify the observability stack is working:

1. **Start the environment:**
   ```bash
   make up-fullstack  # or make up-dev
   ```

2. **Check Jaeger UI:**
   - Open http://localhost:16686
   - You should see the Jaeger UI

3. **Send a test trace:**
   ```bash
   # Using grpcurl or similar tool
   # This should create a trace visible in Jaeger UI
   ```

4. **Verify no port conflicts:**
   ```bash
   docker compose ps
   # All services should be "Up" with no port allocation errors
   ```

## Architecture Decisions

### Why Two Different Architectures?

1. **Backend Template (All-in-One):**
   - Simpler setup with fewer moving parts
   - Suitable for APIs that primarily send traces
   - Single container reduces resource overhead
   - Easier for developers to understand

2. **Web/Fullstack Templates (Modular):**
   - More flexibility for advanced configuration
   - Can swap out Jaeger for other backends (Zipkin, Tempo, etc.)
   - OpenTelemetry Collector can process/filter traces before sending
   - Better separation of concerns
   - More representative of production architectures

Both approaches are valid and appropriate for their respective use cases.

## References

- [OpenTelemetry Collector Documentation](https://opentelemetry.io/docs/collector/)
- [Jaeger Documentation](https://www.jaegertracing.io/docs/)
- [OTLP Specification](https://opentelemetry.io/docs/specs/otlp/)
