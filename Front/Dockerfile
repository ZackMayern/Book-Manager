FROM node:22-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY . .
RUN npm run build -- --configuration production

FROM nginx:1.27-alpine AS runtime
RUN cat <<'EOF' > /etc/nginx/conf.d/default.conf
server {
	listen 80;
	server_name _;

	root /usr/share/nginx/html;
	index index.html;

	# Forward API requests to the backend service inside the cluster.
	location /api/ {
		proxy_pass http://book-manager-api-service.book-manager.svc.cluster.local/api/;
		proxy_http_version 1.1;
		proxy_set_header Host $host;
		proxy_set_header X-Real-IP $remote_addr;
		proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
		proxy_set_header X-Forwarded-Proto $scheme;
	}

	# SPA fallback so deep links resolve to Angular routes.
	location / {
		try_files $uri $uri/ /index.html;
	}
}
EOF
COPY --from=build /app/dist/front/browser /usr/share/nginx/html

EXPOSE 80