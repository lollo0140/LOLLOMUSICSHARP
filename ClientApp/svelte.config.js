import adapter from '@sveltejs/adapter-static';

/** @type {import('@sveltejs/kit').Config} */
const config = {
    // Svelte 5 gestisce la compilazione base autonomamente
    kit: {
        adapter: adapter({
            pages: '../wwwroot',
            assets: '../wwwroot',
            fallback: 'index.html',
            precompress: false,
            strict: true
        })
    }
};

export default config;
