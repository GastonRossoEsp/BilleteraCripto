<template>
  <section class="form-panel">
    <h2>{{ editando ? "Editar cliente" : "Registrar cliente" }}</h2>

    <form @submit.prevent="guardar">
      <div>
        <label for="nombre">Nombre</label>
        <input id="nombre" v-model.trim="formulario.nombre" type="text" maxlength="100" required placeholder="Nombre del cliente"/>
      </div>
      <div>
        <label for="email">Correo electronico</label>
        <input id="email" v-model.trim="formulario.email" type="email" maxlength="254" required placeholder="cliente@correo.com"/>
      </div>
      <p v-if="editando">Saldo actual: <strong>{{ formatearSaldo(clienteEditar.saldo) }}</strong></p>
      <p v-if="error" role="alert">{{ error }}</p>

      <button type="submit" :disabled="guardando">{{ guardando ? "Guardando..." : editando ? "Guardar cambios" : "Registrar cliente" }}</button>
      <button v-if="editando" type="button" @click="cancelar" :disabled="guardando">Cancelar</button>
    </form>
  </section>
</template>

<script setup>
import { ref, computed, reactive, watch } from 'vue'
import { useClientesStore } from '@/stores/clientes.store'

const props = defineProps({
  clienteEditar: {
    type: Object,
    default: null
  }
})

const emit = defineEmits(["guardado", "cancelar"])
const clientesStore = useClientesStore()
const guardando = ref(false)
const error = ref("")

const formulario = reactive({
  nombre: "",
  email: ""
})

const editando = computed(() => props.clienteEditar !== null)

watch(() => props.clienteEditar, (cliente) => {
  error.value = ""
  if (cliente) {
    formulario.nombre = cliente.nombre ?? ""
    formulario.email = cliente.email ?? ""
  } else {
    formulario.nombre = ""
    formulario.email = ""
  }
}, { immediate: true })

const formatearSaldo = (saldo) => {
  return Number(saldo ?? 0).toLocaleString("es-AR", { style: "currency", currency: "ARS" })
}

const guardar = async () => {
  error.value = ""
  const nombre = formulario.nombre.trim()
  const email = formulario.email.trim()
  if (!nombre) {
    error.value = "El nombre es obligatorio"
    return
  }
  if (!email) {
    error.value = "El correo electrónico es obligatorio"
    return
  }
  const formatoEmail = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  if (!formatoEmail.test(email)) {
    error.value = "El correo electrónico no tiene un formato válido"
    return
  }
  guardando.value = true
  try {
    if (editando.value) {
      const clienteActual = await clientesStore.obtenerPorId(props.clienteEditar.id)
      if (!clienteActual) {
        throw new Error("No se pudo recuperar el cliente para editar.")
      }
      await clientesStore.actualizar(props.clienteEditar.id, {id: props.clienteEditar.id, nombre, email, saldo: Number(clienteActual.saldo ?? 0)})
    } else {
      await clientesStore.crear({ nombre, email, saldo: 0 })
    }
    emit("guardado")
  } catch (err) {
    console.error("Error guardando cliente:", err)
    const data = err.response?.data
    if (typeof data === "string") { error.value = data 
    } else if (data && typeof data === "object") { error.value = data.mensaje || data.message || data.title || "Ocurrió un error al guardar el cliente."
    } else if (!err.response) { error.value = "No se pudo conectar con el servidor. Por favor, verifique su conexión a internet."
    } else { error.value = "Ocurrió un error al guardar el cliente." }
  } finally {
    guardando.value = false
  }
}

const cancelar = () => {
  error.value = ""
  emit("cancelar")
}
</script>

<style scoped>
.form-panel {
  background: white;
  padding: 22px;
  border: 1px solid #e0e4e8;
  border-radius: 6px;
  margin-bottom: 24px;
}

.form-panel h2 {
  margin-bottom: 18px;
}

.form-panel form > div {
  margin-bottom: 10px;
}

.form-panel input {
  display: block;
}

.form-panel button + button {
  background-color: #6c757d;
}

.form-panel button + button:hover {
  background-color: #555e65;
}

.form-panel [role="alert"] {
  color: #b42318;
  margin: 10px 0;
}
</style>