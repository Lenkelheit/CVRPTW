FROM node:11.6.0-alpine AS builder
COPY . ./Angular
WORKDIR /Angular
RUN npm i
RUN $(npm bin)/ng build --prod

FROM nginx:1.15.8-alpine
COPY --from=builder /Angular/dist/Angular/ /usr/share/nginx/html
COPY --from=builder /Angular/nginx.conf /etc/nginx/nginx.conf