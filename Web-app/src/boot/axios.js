import axios from 'axios';
import { boot } from 'quasar/wrappers';

const api = axios.create({
  baseURL: 'https://localhost:5008/api',
  //baseURL: 'https://public.ehealth.by/lab-test/api/integration',  // for testing (no results)
  timeout: 1000,
});

export default boot(({ app }) => {
  app.config.globalProperties.$axios = axios;
  app.config.globalProperties.$api = api;
});

export { api };
