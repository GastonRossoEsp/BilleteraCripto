<template>
  <input v-model="transaccion.clienteId" type="number" placeholder="ID Cliente" />
  <input v-model="transaccion.codigoCripto" />
  <input v-model="transaccion.cantCripto" type="number" />
  <button @click="guardar">Guardar</button>
</template>

<script setup>
import { ref } from 'vue'
import { useTransaccionesStore } from '@/stores/transacciones.store'

const store = useTransaccionesStore()

const transaccion = ref({
  clienteId: 0,
  metodo: 'purchase',
  codigoCripto: 'bitcoin',
  cantCripto: 0,
  datetime: new Date().toISOString().slice(0,16)
})

const guardar = async () => {
  await store.crear(transaccion.value)
}
</script>