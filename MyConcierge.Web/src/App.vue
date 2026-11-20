<script setup>
import { ref } from 'vue'
import UnitesPage from './pages/UnitesPage.vue'
import ContratsPage from './pages/ContratsPage.vue'

const onglet = ref('unites')
</script>

<template>
  <q-layout view="hHh lpR fFf">
    <q-header elevated>
      <q-toolbar>
        <q-toolbar-title>MyConcierge</q-toolbar-title>
      </q-toolbar>
      <q-tabs v-model="onglet" align="left">
        <q-tab name="unites" label="Unités" />
        <q-tab name="contrats" label="Contrats" />
      </q-tabs>
    </q-header>

    <q-page-container>
      <q-page padding>
        <!-- v-if : la page se recharge à chaque changement d'onglet -->
        <UnitesPage v-if="onglet === 'unites'" />
        <ContratsPage v-else />
      </q-page>
    </q-page-container>
  </q-layout>
</template>
