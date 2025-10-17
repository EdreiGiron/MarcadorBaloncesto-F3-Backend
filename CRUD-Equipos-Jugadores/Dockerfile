FROM composer:2 AS vendor
WORKDIR /app

COPY composer.json composer.lock ./
ENV COMPOSER_ALLOW_SUPERUSER=1 \
    COMPOSER_MEMORY_LIMIT=-1

RUN composer install --no-dev --prefer-dist --no-interaction --no-progress --optimize-autoloader --no-scripts

COPY . .

RUN composer dump-autoload -o

FROM php:8.3-fpm-alpine AS app
WORKDIR /var/www/html

RUN set -ex \
    && apk add --no-cache bash icu-dev oniguruma-dev libzip-dev curl git procps \
    && docker-php-ext-install pdo_mysql opcache \
    && rm -rf /var/cache/apk/*

COPY --from=vendor /app /var/www/html

RUN chown -R www-data:www-data storage bootstrap/cache \
    && find storage -type d -exec chmod 775 {} \; \
    && find storage -type f -exec chmod 664 {} \; \
    && chmod -R 775 bootstrap/cache

COPY docker/entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

EXPOSE 9000
ENTRYPOINT ["/entrypoint.sh"]
CMD ["php-fpm", "-F"]
