import axios from 'axios';
import { boot } from 'quasar/wrappers';

const api = axios.create({
  //baseURL: 'http://localhost:5008/weblab/',        //error w/ url for API
  baseURL: 'https://public.ehealth.by/lab-test/api/integration',  // for testing (no results, 401 Unauthorized)
  timeout: 1000,
});

export default boot(({ app }) => {
  app.config.globalProperties.$axios = axios;
  app.config.globalProperties.$api = api;
});

export { api };
