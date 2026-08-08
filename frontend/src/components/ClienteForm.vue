<template>
  <div class="form">
    <h3>Nuevo cliente</h3>
    <input v-model="cliente.nombre" placeholder="Nombre" />
    <input v-model="cliente.email" placeholder="Email" />
    <button @click="guardar">Guardar</button>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useClientesStore } from '@/stores/clientes.store'

const store = useClientesStore()

const cliente = ref({
  nombre: '',
  email: ''
})

const guardar = async () => {
  await store.crear(cliente.value)
  cliente.value ={
    nombre: '',
    email: '' 
  }
}
</script>