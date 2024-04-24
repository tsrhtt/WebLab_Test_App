import axios from 'axios';
import { boot } from 'quasar/wrappers';

const api = axios.create({
  baseURL: 'http://localhost:5008/weblab',
  //baseURL: 'https://public.ehealth.by/lab-test/api/integration', // for initial testing, does not work because of CORS
  timeout: 1000,
});

export default boot(({ app }) => {
  api.interceptors.request.use(
    (config) => {
      console.log("111")
      const token = localStorage.getItem('token');
      if (token) {
        config.headers['Authorization'] = `Bearer ${token.replace(/"/g, '')}`;
      }
      return config;
    },
    (error) => {
      Promise.reject(error);
    }
  );
  app.config.globalProperties.$axios = axios;
  app.config.globalProperties.$api = api;
});

export { api };
