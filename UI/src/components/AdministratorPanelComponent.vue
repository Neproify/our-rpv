<template>
  <div
    v-if="isShown"
    id="login"
    class="uk-background-secondary uk-light uk-position-center uk-padding"
  >
    <div :class="currentAnimation">
      <ul uk-tab>
        <li class="uk-active">
          <a href="#" @click="setTab('summary')">Ogólne</a>
        </li>
        <li><a href="#" @click="setTab('players')">Gracze</a></li>
      </ul>
      <SummaryComponent v-if="this.currentTab == 'summary'" />
    </div>
  </div>
</template>

<script>
import SummaryComponent from "./administrator/SummaryAdministratorPanelComponent.vue";

export default {
  name: "AdministratorPanelComponent",
  components: { SummaryComponent },
  mounted: function () {
    window.emitter.on("showAdminWindow", () => {
      this.showWindow = true;
    });
    window.emitter.on("hideAdminWindow", () => {
      this.showWindow = false;
    });
  },
  data: function () {
    return {
      isShown: false,
      currentAnimation: "",
      currentTab: "summary",
    };
  },
  methods: {
    setTab: function (name) {
      this.currentTab = name;
    },
  },
  props: {
    showWindow: false,
  },
  watch: {
    showWindow: function (value) {
      if (value == this.isShown) return;
      if (value == true) {
        this.currentAnimation = "uk-animation-scale-up";
        this.isShown = true;
        setTimeout(
          function (self) {
            self.currentAnimation = "";
          },
          500,
          this
        );
      } else {
        this.currentAnimation = "uk-animation-scale-up uk-animation-reverse";
        setTimeout(
          function (self) {
            self.isShown = false;
            self.currentAnimation = "";
          },
          500,
          this
        );
      }
    },
  },
};
</script>
