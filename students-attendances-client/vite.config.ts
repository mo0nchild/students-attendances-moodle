import { defineConfig, type AliasOptions } from 'vite'
import react from '@vitejs/plugin-react'
import { VitePWA, type VitePWAOptions } from 'vite-plugin-pwa'
import path from 'path'
import fs from 'fs'

const PWAOptions: Partial<VitePWAOptions> = { 
	includeAssets: ["assets/*"],
	registerType: 'autoUpdate',
	manifest: {
        name: 'Посещения студентов RFID',
        short_name: 'Посещения студентов',
        start_url: '/',
        background_color: '#ffffff',
        theme_color: '#000000',
        icons: [
          {
            src: 'icon192.png',
            sizes: '192x192',
            type: 'image/png'
          },
          {
            src: '/icon512.png',
            sizes: '512x512',
            type: 'image/png'
          }
        ]
	},
	workbox: {
		globPatterns: ["**/*.{js,css,html,ico,png,svg,webmanifest}"],
		navigateFallback: '/index.html',
		runtimeCaching: [
			{
				urlPattern: /\/index\.html/,
				handler: 'NetworkFirst',
				options: {
					cacheName: 'html-cache'
				}
			},
			{
				urlPattern: /\.(?:js|css|ico|png|svg|webmanifest)$/,
				handler: 'CacheFirst',
				options: {
					cacheName: 'static-resources'
				}
			},
			// {
			// 	urlPattern: /^http:\/\/localhost:5102\//,
			// 	handler: 'NetworkOnly',
			// 	options: {
			// 		cacheName: 'aspnet-core-api'
			// 	}
			// }
		],
		skipWaiting: true,
    	clientsClaim: true,
	},
	devOptions: {
		enabled: true 
	}
}

const pathAlias: AliasOptions = [
  	{
		find: "@components",
		replacement: path.resolve(__dirname, "src/components"),
	},
	{
		find: "@pages",
		replacement: path.resolve(__dirname, "src/pages"),
	},
	{
		find: "@services",
		replacement: path.resolve(__dirname, "src/services"),
	},
	{
		find: "@core",
		replacement: path.resolve(__dirname, "src"),
	},
	{
		find: "@styles",
		replacement: path.resolve(__dirname, "src/styles"),
	},
	{
		find: '@utils',
		replacement: path.resolve(__dirname, 'src/utils')
	},
	{
		find: '@models',
		replacement: path.resolve(__dirname, 'src/models')
	},
	{
		find: '@hooks',
		replacement: path.resolve(__dirname, 'src/hooks')
	}
]

// https://vite.dev/config/
export default defineConfig(() => ({
  plugins: [
		react(),
		VitePWA(PWAOptions),
	],
	server: {
		port: 5104,
		https: {
			cert: fs.readFileSync(path.resolve(__dirname, 'certificates/vite.crt')),
			key: fs.readFileSync(path.resolve(__dirname, 'certificates/vite.key')),
		},
	},
	resolve: {
		alias: pathAlias
	}
}))
