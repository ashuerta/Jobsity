#
# RabbitMQ Dockerfile
#
# https://github.com/dockerfile/rabbitmq
#

# Pull base image.
FROM rabbitmq:3.8 AS jobsityrabbitmq

# Install RabbitMQ.
RUN rabbitmq-plugins enable --offline rabbitmq_management
RUN rabbitmq-plugins enable --offline rabbitmq_management_agent
RUN rabbitmq-plugins enable --offline rabbitmq_shovel
RUN rabbitmq-plugins enable --offline rabbitmq_shovel_management
RUN rabbitmq-plugins enable --offline rabbitmq_top
RUN rabbitmq-plugins enable --offline rabbitmq_top_top
RUN rabbitmq-plugins enable --offline rabbitmq_federation_management
RUN rabbitmq-plugins enable --offline rabbitmq_stomp
RUN rabbitmq-plugins enable --offline rabbitmq_web_stomp
RUN rabbitmq-plugins enable --offline rabbitmq_mqtt
RUN rabbitmq-plugins enable --offline rabbitmq_web_mqtt
RUN rabbitmq-plugins enable --offline rabbitmq_web_mqtt_examples
RUN rabbitmq-plugins enable --offline rabbitmq_web_stomp_examples
RUN rabbitmq-plugins enable --offline rabbitmq_auth_backend_http


# Expose ports.
EXPOSE 4369 5672 25672 15670 15672 15674 1883 61613
