FROM node:lts-alpine
WORKDIR /app
COPY package*.json ./
COPY jsconfig.json ./
COPY svelte.config.js ./
COPY postcss.config.cjs ./
COPY tailwind.config.cjs ./
RUN npm install
COPY . .
EXPOSE 3000
EXPOSE 24678
CMD ["npm", "run", "dev"]