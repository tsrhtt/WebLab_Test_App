<template>
  <div>
    <button @click="getDirections">Get Directions</button>
    <table>
      <thead>
        <tr>
          <th>Id</th>
          <th>Card Number</th>
          <th>Patient Full Name</th>
          <th>Birth Date</th>
          <th>Sex Description</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="direction in directions" :key="direction.id">
          <td>{{ direction.id }}</td>
          <td>{{ direction.cardNumber }}</td>
          <td>{{ direction.patientFullName }}</td>
          <td>{{ direction.birthDate }}</td>
          <td>{{ direction.sexDescription }}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script>
import axios from "axios";

export default {
  data() {
    return {
      directions: [],
    };
  },
  methods: {
    async getDirections() {
      // Make a GET request to API to get the directions
      const response = await axios.get(
        "https://public.ehealth.by/lab-test/api/integration/Direction"
      );
      // Set the directions data to a local variable or state
      this.directions = response.data;
    },
  },
};
</script>
