import Keycloak from 'keycloak-js'
import { boot } from 'quasar/wrappers'

  const keycloakConfig = {
    url: process.env.KEYCLOAK_HOST,
    realm: process.env.KEYCLOAK_REALM,
    clientId: process.env.KEYCLOAK_CLIENT_ID,
    onLoad: process.env.KEYCLOAK_ON_LOAD
  }

  const keycloak = new Keycloak(keycloakConfig)

  export default boot(({ app, router, store }) => {
    localStorage.removeItem('token')
    return new Promise((resolve, reject) => {
    keycloak.init({ onLoad: keycloakConfig.onLoad })
      .then((authenticated) => {
        if (authenticated) {
          resolve(keycloak)
        } else {
          reject(new Error('User authentication failed'))
        }
      })
      .catch((error) => {
        reject(error)
      })
      localStorage.setItem('token', keycloak.token)
  })
})

export { keycloak }
