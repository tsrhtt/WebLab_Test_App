import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5000/api', // need to change this to API URL later
  timeout: 1000,
});

export default async ({ Vue }) => {
  Vue.prototype.$axios = axios;
};

export { api };
