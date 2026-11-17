<template>
    <section class="grafico-container">
        <h3>Distribución de la cartera</h3>
        <div v-if="criptomonedas.length > 0" class="grafico">
            <Pie :data="chartData" :options="chartOptions" />
        </div>
        <p v-else>No hay criptomonedas en la cartera para mostrar el gráfico.</p>
    </section>
</template>

<script setup>
import { computed } from 'vue'
import { Pie } from 'vue-chartjs'
import { Chart as ChartJS, Tooltip, Legend, ArcElement } from 'chart.js'

ChartJS.register(Tooltip, Legend, ArcElement)

const props = defineProps({
    criptomonedas: {
        type: Array,
        default: () => []
    }
})

const chartData = computed(() => ({
    labels: props.criptomonedas.map(c => c.codigoCripto.toUpperCase()),
    datasets: [{
        label: 'Valor actual (ARS)',
        data: props.criptomonedas.map(c => c.dinero),
        backgroundColor: ['#315b83', '#4f86b5', '#8bb9e8'],
        borderWidth: 2,
        borderColor: '#ffffff'
    }]
}))

const chartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
        legend: {
            position: 'bottom'
        },
        tooltip: {
            callbacks: {
                label(context) {
                    const valor = context.raw.toLocaleString('es-AR', { style: 'currency', currency: 'ARS' })
                    return `${context.label}: ${valor}`
                }
            }
        }
    }
}
</script>

<style scoped>
.grafico-container {
    background: white;
    border: 1px solid #e0e4e8;
    border-radius: 6px;
    padding: 20px;
    margin: 22px 0;
    max-width: 600px;
}

.grafico-container h3 {
    margin-top: 0;
    margin-bottom: 18px;
}

.grafico {
    position: relative;
    height: 300px;
}
</style>