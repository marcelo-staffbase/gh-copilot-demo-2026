<template>
  <div class="app">
    <header class="header">
      <div class="header-content">
        <div class="header-text">
          <h1>🎵 {{ $t('header.title') }}</h1>
          <p>{{ $t('header.subtitle') }}</p>
        </div>
        <div class="language-selector">
          <label for="language">{{ $t('language.label') }}:</label>
          <select id="language" v-model="currentLocale" @change="changeLanguage">
            <option value="en">{{ $t('language.en') }}</option>
            <option value="fr">{{ $t('language.fr') }}</option>
            <option value="de">{{ $t('language.de') }}</option>
          </select>
        </div>
      </div>
    </header>

    <main class="main">
      <div v-if="loading" class="loading">
        <div class="spinner"></div>
        <p>{{ $t('loading.message') }}</p>
      </div>

      <div v-else-if="error" class="error">
        <p>{{ $t('error.message') }}</p>
        <button @click="fetchAlbums" class="retry-btn">{{ $t('error.retry') }}</button>
      </div>

      <div v-else class="albums-grid">
        <AlbumCard 
          v-for="album in albums" 
          :key="album.id" 
          :album="album" 
        />
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import axios from 'axios'
import AlbumCard from './components/AlbumCard.vue'
import type { Album } from './types/album'

const { locale } = useI18n()
const currentLocale = ref<string>(locale.value)

const albums = ref<Album[]>([])
const loading = ref<boolean>(true)
const error = ref<string | null>(null)

/** 
 * Changes the application language
 */
const changeLanguage = (): void => {
  locale.value = currentLocale.value
}

/** 
 * Fetches the list of albums from the API.
 * Handles loading and error states.
 * @return {Promise<void>}
 */
const fetchAlbums = async (): Promise<void> => {
  try {
    loading.value = true
    error.value = null
    const response = await axios.get<Album[]>('/albums')
    albums.value = response.data
  } catch (err) {
    error.value = 'Failed to load albums. Please make sure the API is running.'
    console.error('Error fetching albums:', err)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchAlbums()
})
</script>

<style scoped>
.app {
  min-height: 100vh;
  padding: 2rem;
}

.header {
  text-align: center;
  margin-bottom: 3rem;
  color: white;
}

.header-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1.5rem;
}

.header-text h1 {
  font-size: 3rem;
  margin-bottom: 0.5rem;
  text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.3);
}

.header-text p {
  font-size: 1.2rem;
  opacity: 0.9;
}

.language-selector {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  background: rgba(255, 255, 255, 0.1);
  padding: 0.75rem 1.5rem;
  border-radius: 25px;
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.language-selector label {
  font-size: 0.95rem;
  font-weight: 500;
}

.language-selector select {
  background: rgba(255, 255, 255, 0.9);
  color: #667eea;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 15px;
  font-size: 0.95rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
  outline: none;
}

.language-selector select:hover {
  background: white;
  transform: translateY(-2px);
}

.language-selector select:focus {
  box-shadow: 0 0 0 3px rgba(255, 255, 255, 0.3);
}

.main {
  max-width: 1200px;
  margin: 0 auto;
}

.loading {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem;
  color: white;
}

.spinner {
  width: 50px;
  height: 50px;
  border: 4px solid rgba(255, 255, 255, 0.3);
  border-top: 4px solid white;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 1rem;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.error {
  text-align: center;
  padding: 4rem;
  color: white;
}

.error p {
  font-size: 1.2rem;
  margin-bottom: 2rem;
}

.retry-btn {
  background: rgba(255, 255, 255, 0.2);
  color: white;
  border: 2px solid white;
  padding: 0.75rem 2rem;
  border-radius: 25px;
  font-size: 1rem;
  cursor: pointer;
  transition: all 0.3s ease;
}

.retry-btn:hover {
  background: white;
  color: #667eea;
}

.albums-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 2rem;
  padding: 1rem;
}

@media (max-width: 768px) {
  .app {
    padding: 1rem;
  }
  
  .header h1 {
    font-size: 2rem;
  }
  
  .albums-grid {
    grid-template-columns: 1fr;
    gap: 1rem;
  }
}
</style>
