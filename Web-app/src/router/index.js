import { createRouter, createWebHistory } from 'vue-router'
import TokenPage from '../views/TokenPage.vue'
import DetailedPage from '../views/DetailedPage.vue'

const routes = [
  { path: '/', name: 'TokenPage', component: TokenPage },
  {
    path: '/detailed/:id',
    name: 'DetailedPage',
    component: DetailedPage,
    props: route => ({ detailedData: JSON.parse(route.query.detailedData) })
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
