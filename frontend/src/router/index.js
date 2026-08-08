import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '@/views/HomeView.vue'
import Transacciones from '@/views/Transacciones.vue'
import Clientes from '@/views/Clientes.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView,
    },
    {
      path: '/transacciones',
      name: 'transacciones',
      component: Transacciones
    },
    {
      path: '/clientes',
      name: 'clientes',
      component: Clientes
    }
  ],
})

export default router
