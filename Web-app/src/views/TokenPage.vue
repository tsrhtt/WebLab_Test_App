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
export default {
  data() {
    return {
      directions: [],
    };
  },
  methods: {
    async getDirections() {
      try {
        // Using the globally configured Axios instance $api
        const response = await this.$api.get("Direction/");
        this.directions = response.data;
      } catch (error) {
        console.error('Failed to fetch directions:', error);
        alert('Failed to fetch directions. Please check the console for more details.');
      }
    },
  },
};
</script>
