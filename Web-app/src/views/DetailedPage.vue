<template>
  <div class="detailed-page">
    <h1>Подробные данные направления</h1>
    <pre>{{ detailedData }}</pre>
  </div>
</template>

<script>
export default {
  props: {
    detailedData: {
      type: Object,
      required: true,
    },
  },
};
</script>

<style scoped>
.detailed-page {
  padding: 20px;
  background-color: #f0f8ff;
  border-radius: 20px;
  font-family: 'Arial', sans-serif;
}

.detailed-page h1 {
  color: #ADD8E6;
  font-size: 24px;
  font-weight: bold;
  margin-bottom: 20px;
}

.detailed-page pre {
  background-color: #fff;
  padding: 20px;
  border-radius: 10px;
  overflow: auto;
  max-height: 400px;
}
</style>
