<template>
  <div class="app-container">
    <aside class="navigation">
      <div class="nav-item" @click="goToMainPage">
        <i class="fas fa-arrow-left"></i>
        <span>Назад</span>
      </div>
      <div class="nav-item">
        <i class="fas fa-vial"></i>
        <span>Лаборатория</span>
      </div>
    </aside>

    <main class="content">
      <header class="header">
        <h1>ЛАБОРАТОРИЯ.WEB</h1>
      </header>

      <div class="main-content">
        <div class="tables">
          <table class="styled-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Полное имя пациента</th>
                <th>Тип анализа</th>
                <th>Тип биоматериала</th>
                <th>Количество биоматериала</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="direction in directions" :key="direction.id" @click="selectDirection(direction)" :style="rowStyle(direction)">
                <td>{{ direction.id }}</td>
                <td>{{ direction.patientFullName }}</td>
                <td>{{ direction.analysTypeName }}</td>
                <td>{{ direction.bioMaterialType }}</td>
                <td>{{ direction.bioMaterialCount }}</td>
              </tr>
            </tbody>
          </table>
        </div>

        <div class="emulator">
          <div v-if="!selectedDirection" class="placeholder">
            <p>Пожалуйста, выберите анализ для эмуляции</p>
          </div>
          <div v-else>
            <div v-for="indicator in indicators" :key="indicator.id" class="indicator-card">
              <h3>{{ indicator.name }}</h3>
              <p>{{ indicator.abbreviation }}</p>
              <p>{{ indicator.group }}</p>
              <div class="emulator-details">
                <div>
                  <p>Требуемое количество биоматериала: {{ indicator.requiredBioMaterialCount }}</p>
                  <p v-if="indicator.resultStr || indicator.resultVal">
                    Результат: {{ indicator.resultStr || indicator.resultVal }} {{ indicator.units }}
                  </p>
                  <p v-else>
                    <input type="text" v-model="indicator.result" placeholder="Результат">
                    <span>{{ indicator.units }}</span>
                  </p>
                </div>
                <button v-if="!indicator.resultStr && !indicator.resultVal" :disabled="!canSimulate(indicator)" @click="simulateResult(indicator, $event)">Эмулировать</button>
              </div>
              <div class="progress-bar" :style="indicator.progressBarStyle"></div>
            </div>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<script>
export default {
  data() {
    return {
      directions: [],
      selectedDirection: null,
      indicators: []
    };
  },
  methods: {
    async fetchDirections() {
      try {
        const response = await this.$api.get('direction/fromdb');
        if (response.data && Array.isArray(response.data)) {
          this.directions = response.data.filter(d => d.directionStatusId === 6); // Only directions with status 6
        } else if (response.data && response.data.$values) {
          this.directions = response.data.$values.filter(d => d.directionStatusId === 6);
        } else {
          console.error('Unexpected response format:', response.data);
        }
      } catch (error) {
        console.error('Failed to fetch directions:', error);
      }
    },
    async selectDirection(direction) {
      this.selectedDirection = direction;
      await this.fetchIndicators(direction.id);
    },
    async fetchIndicators(directionId) {
      try {
        const response = await this.$api.get(`direction/detailed/${directionId}`);
        const detailedData = response.data;
        if (detailedData.indicators && detailedData.indicators.$values) {
          this.indicators = detailedData.indicators.$values.map(indicator => ({
            ...indicator,
            requiredBioMaterialCount: this.randomBioMaterialCount(),
            result: null,
            progressBarStyle: {
              width: '0%',
              background: 'linear-gradient(to right, red, green)',
              borderRadius: '5px',
              height: '10px',
              transition: 'width 5s'
            }
          }));
        } else {
          console.error('Unexpected indicators format:', detailedData.indicators);
        }
      } catch (error) {
        console.error('Failed to fetch indicators:', error);
      }
    },
    randomBioMaterialCount() {
      return Math.floor(Math.random() * 4) + 1;
    },
    canSimulate(indicator) {
      return this.selectedDirection.bioMaterialCount >= indicator.requiredBioMaterialCount;
    },
    async simulateResult(indicator, event) {
      event.target.disabled = true;
      indicator.progressBarStyle.width = '100%';
      setTimeout(async () => {
        if (indicator.type === 1) {
          indicator.resultVal = Math.floor(Math.random() * 5000);
          if (indicator.isNormExist) {
            indicator.progressBarStyle.background = (indicator.resultVal >= indicator.minStandardValue && indicator.resultVal <= indicator.maxStandardValue) ? 'green' : 'red';
          } else {
            indicator.progressBarStyle.background = 'green';
          }
        } else if (indicator.type === 2) {
          const randomIndex = Math.floor(Math.random() * indicator.possibleStringValues.$values.length);
          indicator.resultStr = indicator.possibleStringValues.$values[randomIndex];
          if (indicator.isNormExist) {
            indicator.progressBarStyle.background = Array.isArray(indicator.textStandards.$values) && indicator.textStandards.$values.includes(indicator.resultStr) ? 'green' : 'red';
          } else {
            indicator.progressBarStyle.background = 'green';
          }
        }
        this.selectedDirection.bioMaterialCount -= indicator.requiredBioMaterialCount;
        await this.updateDirectionBioMaterialCount(this.selectedDirection);
      }, 5000);
    },
    async updateDirectionBioMaterialCount(direction) {
      try {
        await this.$api.put(`direction/${direction.id}`, direction);
      } catch (error) {
        console.error('Failed to update direction bio material count:', error);
      }
    },
    rowStyle(direction) {
      if (this.selectedDirection && this.selectedDirection.id === direction.id) {
        return { backgroundColor: '#98FB9866' };
      }
      return {};
    },
    goToMainPage() {
      this.$router.push({ name: 'TokenPage' });
    }
  },
  async mounted() {
    await this.fetchDirections();
  }
};
</script>

<style scoped>
.app-container {
  display: flex;
  height: 100vh;
  font-family: 'Arial', sans-serif;
}

.navigation {
  width: 80px;
  background-color: #ADD8E6;
  padding: 20px 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  position: fixed;
  top: 0;
  bottom: 0;
  left: 0;
}

.nav-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-bottom: 20px;
  color: white;
  font-size: 14px;
}

.nav-item i {
  font-size: 24px;
  margin-bottom: 5px;
  cursor: pointer;
}

.content {
  flex-grow: 1;
  padding: 20px;
  margin-left: 100px;
  background-color: #f4f7f6;
  border-top-left-radius: 20px;
  overflow-y: auto;
}

.header {
  display: flex;
  justify-content: center;
  align-items: center;
  margin-bottom: 20px;
  height: 50px;
}

.header h1 {
  color: #ADD8E6;
  font-size: 24px;
  font-weight: bold;
}

.main-content {
  display: flex;
}

.tables {
  width: 50%;
  background-color: white;
  padding: 20px;
  border-radius: 20px;
  margin-right: 20px;
}

.styled-table {
  width: 100%;
  border-collapse: collapse;
  margin-bottom: 20px;
  border-radius: 20px;
  overflow: hidden;
  box-shadow: 0 0 20px rgba(0, 0, 0, 0.1);
}

.styled-table th,
.styled-table td {
  padding: 12px 15px;
  text-align: left;
}

.styled-table th {
  background-color: #ADD8E6;
  color: white;
}

.styled-table tbody tr:nth-of-type(even) {
  background-color: #f3f3f3;
}

.styled-table tbody tr {
  border-bottom: 1px solid #dddddd;
}

.styled-table tbody tr:last-of-type {
  border-bottom: 2px solid #ADD8E6;
}

.emulator {
  width: 50%;
  text-align: center;
}

.placeholder {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100%;
}

.placeholder p {
  color: #ccc;
  font-size: 18px;
}

.indicator-card {
  background-color: white;
  border: 1px solid #ddd;
  border-radius: 10px;
  padding: 20px;
  margin-bottom: 20px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.indicator-card h3 {
  margin: 0;
  font-size: 18px;
  font-weight: bold;
  color: #333;
}

.indicator-card p {
  margin: 5px 0;
  font-size: 14px;
  color: #666;
}

.emulator-details {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.emulator-details button {
  background-color: #ADD8E6;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 20px;
  cursor: pointer;
  font-size: 16px;
}

.emulator-details button:disabled {
  background-color: #ccc;
  cursor: not-allowed;
}

.progress-bar {
  width: 100%;
  border-radius: 5px;
  height: 5px;
  background: linear-gradient(to right, red, yellow, green);
  transition: width 5s;
}
</style>
