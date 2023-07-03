<template>
  <q-page class="flex flex-center">
    <div class="q-pa-md" style="max-width: 300px">
      <q-btn label="Get tests" @click="getTests" />
      <q-table
        :data="tests"
        :columns="columns"
        row-key="name"
        binary-state-sort
        dense
      />
    </div>
  </q-page>
</template>

<script>
import { api } from 'boot/axios';

export default {
  data() {
    return {
      tests: [], // array of test objects
      columns: [
        {
          name: 'name',
          required: true,
          label: 'Name',
          align: 'left',
          field: row => row.name,
          format: val => `${val}`,
          sortable: true,
        },
        {
          name: 'result',
          required: true,
          label: 'Result',
          align: 'left',
          field: row => row.result,
          format: val => `${val}`,
          sortable: true,
        },
      ],
    };
  },
  methods: {
    getTests() {
      api.get('/tests') // get all tests from API
        .then(response => {
          // handle success
          console.log(response.data);
          this.tests = response.data; // assign the data to the tests array
        })
        .catch(error => {
          // handle error
          console.error(error);
        });
    },
  },
};
</script>