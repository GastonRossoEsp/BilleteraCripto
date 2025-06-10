import axios from 'axios'
const API = axios.create({
    baseURL: 'https://localhost:7198/api',
})
export default API