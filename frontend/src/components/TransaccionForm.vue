<template>
  <form @submit.prevent="handleSubmit">
    <select v-model="transaccion.codigoCripto" required>
      <option disabled value="">Seleccionar Cripto</option>
      <option value="bitcoin">Bitcoin</option>
      <option value="usdc">USDC</option>
      <option value="ethereum">Ethereum</option>
    </select>

    <select v-model="transaccion.metodo" required>
      <option value="purchase">Compra</option>
      <option value="sale">Venta</option>
    </select>

    <input type="number" step="0.0001" v-model="transaccion.cantCripto" placeholder="Cantidad" required />
    <input type="number" v-model="transaccion.clienteId" placeholder="Id cliente" required />
    <input type="datetime-local" v-model="transaccion.datetime" required />
    <button type="submit">Registrar</button>
  </form>
</template>

<script setup>
import { ref } from 'vue'
import axios from 'axios'

const transaccion = ref({
  codigoCripto: 'bitcoin',
  metodo: 'purchase',
  clienteId: 1,
  cantCripto: 0,
  datetime: new Date().toISOString().slice(0, 16)
})

const handleSubmit = async() => {
    try {
    await axios.post('https://localhost:7198/api/Transaccion', transaccion.value)
    alert('Transacción registrada correctamente')
  } catch (error) {
    console.error(error)
    alert('Error al registrar la transacción')
  }
}
</script>