
<template>
    <main class="transacciones-view">
        <TransaccionForm :transaccion-editar="transaccionEditar" @guardada="finalizarEdicion" @cancelar="cancelarEdicion"/>
        <TransaccionLista @editar="editarTransaccion" />
    </main>
</template>

<script setup>
import { ref } from 'vue'
import TransaccionForm from '@/components/TransaccionForm.vue'
import TransaccionLista from '@/components/TransaccionLista.vue'

const transaccionEditar = ref(null)
const editarTransaccion = (transaccion) => {
    transaccionEditar.value = transaccion

    window.scrollTo({top: 0, behavior: 'smooth'})
}

const finalizarEdicion = () => { transaccionEditar.value = null }
const cancelarEdicion = () => { transaccionEditar.value = null }
</script>