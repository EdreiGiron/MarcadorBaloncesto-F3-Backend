#!/usr/bin/env bash
set -e


if [ -n "${DB_HOST}" ]; then
  echo "Esperando a la base de datos ${DB_HOST}:${DB_PORT:-3306} ..."
  for i in {1..30}; do
    if php -r "
      try {
        \$dbh = new PDO('mysql:host=${DB_HOST};port=${DB_PORT:-3306};dbname=${DB_DATABASE}', '${DB_USERNAME}', '${DB_PASSWORD}');
        echo 'OK';
      } catch (Throwable \$e) { /* noop */ }
    " | grep -q OK; then
      echo " DB lista."
      break
    fi
    sleep 2
  done
fi

if ! grep -q "APP_KEY=base64:" .env 2>/dev/null; then
  echo "Generando APP_KEY..."
  php artisan key:generate --force
fi

php artisan key:generate --force || true
php artisan config:clear || true
php artisan package:discover --ansi || true
php artisan config:cache || true
php artisan route:cache || true
php artisan migrate --force

exec "$@"
