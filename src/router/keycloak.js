import Keycloak from 'keycloak-js'

export default function () {
  const keycloakConfig = {
    url: process.env.KEYCLOAK_HOST,
    realm: process.env.KEYCLOAK_REALM,
    clientId: process.env.KEYCLOAK_CLIENT_ID,
    onLoad: process.env.KEYCLOAK_ON_LOAD
  }

  localStorage.removeItem('token')

  const keycloak = new Keycloak(keycloakConfig)

  return new Promise((resolve, reject) => {
    keycloak
      .init({ onLoad: keycloakConfig.onLoad })
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
  })
}

