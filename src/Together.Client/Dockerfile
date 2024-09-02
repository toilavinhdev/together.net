# standalone
# docker build --progress=plain --no-cache -t together-client .
# not using BOM format UTF-8 for nginx.conf

FROM node:alpine AS build
EXPOSE 80

WORKDIR /app
COPY ["package*.json", "."]
RUN npm i -g @angular/cli@17
RUN npm i
COPY . .
RUN npm run build:dev

FROM nginx:alpine AS final
COPY ./nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/together.net /usr/share/nginx/html

RUN ls -lha /usr/share/nginx/html
RUN cat /etc/nginx/nginx.conf
RUN cat /etc/nginx/conf.d/default.conf
RUN nginx -t
