FROM php:8.3-fpm

# Установка нужных пакетов и расширений
RUN apt-get update && apt-get install -y \
    nginx \
    curl \
    unzip \
    git \
    supervisor \
    libpng-dev \
    libjpeg-dev \
    libfreetype6-dev \
    libzip-dev \
    libonig-dev \
    libxml2-dev \
    libpq-dev \
    && docker-php-ext-install \
        gd \
        bcmath \
        intl \
        xml \
        soap \
        mbstring \
        zip \
        pdo \
        pdo_mysql \
        mysqli \
        sockets \
        opcache

# Установка Composer
RUN curl -sS https://getcomposer.org/installer | php -- --install-dir=/usr/local/bin --filename=composer

# Установка Moodle
RUN curl -L -o moodle.tgz https://download.moodle.org/download.php/direct/stable405/moodle-latest-405.tgz && \
    tar -xzf moodle.tgz -C /var/www/ && \
    rm moodle.tgz && \
    chown -R www-data:www-data /var/www/moodle

# Установка плагина Attendance
RUN curl -L -o attendance.zip https://moodle.org/plugins/download.php/35587/mod_attendance_moodle45_2024072401.zip && \
    unzip attendance.zip -d /var/www/moodle/mod && \
    rm attendance.zip && \
    chown -R www-data:www-data /var/www/moodle/mod/attendance

RUN mkdir -p /var/www/moodledata \
    && chown -R www-data:www-data /var/www/moodledata

COPY config.php /var/www/moodle/config.php

# Копируем плагин в Moodle
COPY local_plugins/myplugin /var/www/moodle/local/myplugin

# Установка PhpAmqpLib через Composer
RUN cd /var/www/moodle/local/myplugin && composer require php-amqplib/php-amqplib:^3.7.3

RUN chown -R www-data:www-data /var/www/moodle/local/myplugin && \
    chmod -R 755 /var/www/moodle/local/myplugin

# Копируем конфиги
COPY nginx.conf /etc/nginx/sites-available/default
COPY php.ini /usr/local/etc/php/conf.d/custom.ini
COPY supervisord.conf /etc/supervisor/conf.d/supervisord.conf

# Создание необходимых директорий
RUN mkdir -p /run/php && chown www-data:www-data /run/php

WORKDIR /var/www/moodledata
# Порты
EXPOSE 80

CMD ["/usr/bin/supervisord"]
