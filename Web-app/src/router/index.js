import { createRouter, createWebHistory } from 'vue-router'
import TokenPage from '../views/TokenPage.vue'

const routes = [
  {
    path: '/',
    name: 'TokenPage',
    component: TokenPage
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
