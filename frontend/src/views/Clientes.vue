<template>
  <main>
    <ClienteForm :cliente-editar="clienteEditar" @guardado="finalizarEdicion" @cancelar="cancelarEdicion"/>
    <hr />
    <ClienteLista @editar="editarCliente" />
  </main>
</template>


<script setup>
import { ref } from 'vue'
import ClienteForm from '@/components/ClienteForm.vue'
import ClienteLista from '@/components/ClienteLista.vue'

const clienteEditar = ref(null)
const editarCliente = (cliente) => {
  clienteEditar.value = cliente
  window.scrollTo({ top: 0, behavior: 'smooth' })
}
const finalizarEdicion = () => {
  clienteEditar.value = null
}
const cancelarEdicion = () => {
  clienteEditar.value = null
}
</script>