#!/bin/bash
set -e

# === Configuración ===
CONTAINER="oracle-db-dvp"
READY_MESSAGE="DATABASE IS READY TO USE!"
INTERVAL=5
MAX_ATTEMPTS=60

echo "🚀 Iniciando todos los servicios base (Oracle, MongoDB, Rails)..."
docker compose build --no-cache oracle-db-dvp mongodb-dvp audit-service-dvp && docker compose up oracle-db-dvp mongodb-dvp audit-service-dvp -d

echo "⏳ Esperando a que Oracle esté completamente inicializado..."
attempt=1
while [ $attempt -le $MAX_ATTEMPTS ]; do
  if docker logs "$CONTAINER" 2>&1 | tail -n 200 | grep -q "$READY_MESSAGE"; then
    echo "✅ Oracle está listo para recibir conexiones."

    echo "👤 Creando usuario de aplicación (DVP) y otorgando permisos..."
    if ! docker exec -i "$CONTAINER" bash -c "echo \"
      WHENEVER SQLERROR EXIT SQL.SQLCODE;
      DECLARE
        v_count INTEGER;
      BEGIN
        SELECT COUNT(*) INTO v_count FROM all_users WHERE username = 'DVP';
        IF v_count = 0 THEN
          EXECUTE IMMEDIATE 'CREATE USER dvp IDENTIFIED BY "TuContrasena123"';
          EXECUTE IMMEDIATE 'GRANT CONNECT, RESOURCE, CREATE SESSION, CREATE TABLE, CREATE VIEW, CREATE SEQUENCE, CREATE TRIGGER, CREATE PROCEDURE TO dvp';
          EXECUTE IMMEDIATE 'ALTER USER dvp QUOTA UNLIMITED ON USERS';
        END IF;
      END;
      /
    \" | sqlplus -s system/TuContrasena123*@//localhost:1521/XEPDB1"; then
      echo "❌ Error al crear o configurar el usuario DVP. Abortando ejecución."
      exit 1
    fi

    echo "✅ Usuario DVP creado y configurado correctamente."

    echo "🟢 Levantando billing-service-dvp..."
    docker compose build --no-cache billing-service-dvp && docker compose up billing-service-dvp
    exit 0
  fi
  echo "⏳ Intento $attempt/$MAX_ATTEMPTS: Oracle aún no está listo. Reintentando en $INTERVAL s..."
  sleep $INTERVAL
  attempt=$((attempt+1))
done

echo "❌ Oracle no mostró el mensaje esperado tras $((INTERVAL * MAX_ATTEMPTS)) segundos."
exit 1
