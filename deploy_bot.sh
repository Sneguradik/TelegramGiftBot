#!/bin/bash
set -e

# === Конфигурация ===
ROOT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )" # текущая папка скрипта
APP_DIR="/opt/telegram-gifts-bot"
DOTNET_PROJECT_PATH="$ROOT_DIR/Web"
SCREEN_NAME="telegrambot"
DOCKER_COMPOSE_FILE="$ROOT_DIR/docker-compose.yml"

echo "=== 0. Проверка зависимостей ==="

# --- Проверка и установка .NET ---
if ! command -v dotnet &> /dev/null
then
    echo "dotnet не найден. Устанавливаю .NET 9 SDK..."

    if [[ "$OSTYPE" == "linux-gnu"* ]]; then
        wget https://dot.net/v1/dotnet-install.sh -O /tmp/dotnet-install.sh
        chmod +x /tmp/dotnet-install.sh
        /tmp/dotnet-install.sh --channel 9.0
        export PATH=$PATH:$HOME/.dotnet
        echo 'export PATH=$PATH:$HOME/.dotnet' >> ~/.bashrc
    else
        echo "Установка автоматическая поддерживается только на Linux. На macOS поставь .NET через brew:"
        echo "brew install --cask dotnet-sdk"
        exit 1
    fi
fi

dotnet --version

# --- Проверка Docker ---
if ! command -v docker &> /dev/null
then
    echo "Docker не установлен. Установи Docker и перезапусти скрипт."
    exit 1
fi

if ! command -v docker compose &> /dev/null
then
    echo "Docker Compose v2 не найден. Установи плагин Docker Compose."
    exit 1
fi

# --- Проверка screen ---
if ! command -v screen &> /dev/null
then
    echo "Устанавливаю screen..."
    sudo apt-get update && sudo apt-get install -y screen
fi

# === 1. Сборка проекта ===
echo "=== Сборка проекта в $APP_DIR ==="
mkdir -p "$APP_DIR"
dotnet publish "$DOTNET_PROJECT_PATH" -c Release -o "$APP_DIR"

# === 2. Запуск Docker Compose ===
echo "=== Запуск docker-compose ==="
docker compose -f "$DOCKER_COMPOSE_FILE" up -d

# === 3. Запуск бота через screen ===
echo "=== Запуск бота в screen сессии $SCREEN_NAME ==="

# Если старая сессия есть — убиваем
if screen -list | grep -q "$SCREEN_NAME"; then
    echo "Старая сессия $SCREEN_NAME найдена, завершаю..."
    screen -S "$SCREEN_NAME" -X quit || true
fi

# Запуск новой сессии
screen -dmS "$SCREEN_NAME" bash -c "cd $APP_DIR && dotnet Web.dll"

echo "=== Готово ==="
echo "Подключиться к консоли: screen -r $SCREEN_NAME"
echo "Отсоединиться: Ctrl+A D"
echo "Логи docker-compose: docker compose -f $DOCKER_COMPOSE_FILE logs -f"
