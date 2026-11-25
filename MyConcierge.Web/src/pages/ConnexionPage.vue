<script setup>
import { ref } from 'vue'
import { api, afficherErreur } from '../api'
import { ouvrirSession } from '../session'

const courriel = ref('')
const motDePasse = ref('')
const connexionEnCours = ref(false)

async function seConnecter() {
  connexionEnCours.value = true
  try {
    const reponse = await api.post('/auth/connexion', { courriel: courriel.value, motDePasse: motDePasse.value })
    ouvrirSession(reponse)
  } catch (erreur) {
    afficherErreur(erreur)
    motDePasse.value = ''
  } finally {
    connexionEnCours.value = false
  }
}
</script>

<template>
  <q-layout view="hHh lpR fFf" class="bg-grey-2">
    <q-page-container>
      <q-page class="flex flex-center">
        <q-card flat bordered style="width: 360px; max-width: 90vw">
          <q-card-section class="text-center">
            <q-icon name="apartment" color="primary" size="lg" />
            <div class="text-h6 q-mt-sm">MyConcierge</div>
            <div class="text-grey-7">Connecte-toi pour continuer</div>
          </q-card-section>

          <q-form @submit="seConnecter">
            <q-card-section class="q-gutter-md">
              <q-input v-model="courriel" type="email" label="Courriel" dense outlined autofocus
                       autocomplete="username" :rules="[(valeur) => !!valeur || 'Requis']" />
              <q-input v-model="motDePasse" type="password" label="Mot de passe" dense outlined
                       autocomplete="current-password" :rules="[(valeur) => !!valeur || 'Requis']" />
            </q-card-section>
            <q-card-actions class="q-px-md q-pb-md">
              <q-btn type="submit" color="primary" label="Se connecter" class="full-width" :loading="connexionEnCours" />
            </q-card-actions>
          </q-form>
        </q-card>
      </q-page>
    </q-page-container>
  </q-layout>
</template>
