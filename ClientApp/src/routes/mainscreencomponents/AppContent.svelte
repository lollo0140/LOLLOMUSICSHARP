<script>
    import NavigationBar from "./navigation_bar.svelte";
    import PlayerControlls from "./player_controlls.svelte";
    import PlayerDisplay from "./player_display.svelte";
    import LogISection from "./LogISection.svelte";
    import UpperBar from "./UpperBar.svelte";

    let { children } = $props();
</script>

<div class="content">
    <div class="app-content">
        <div class="app-navigator lollo-appstyle-DivContainer">
            <LogISection/>

            <nav class="lollo-appstyle-DivContainer">
                <NavigationBar />
            </nav>

            <div class="upper-bar">
                <UpperBar/>
            </div>

            <div class="content-renderer lollo-appstyle-DivContainer">
                {@render children()}
            </div>
        </div>

        <div class="display lollo-appstyle-DivContainer">
            <PlayerDisplay />
        </div>
    </div>

    <div class="controlls lollo-appstyle-DivContainer">
        <PlayerControlls />
    </div>
</div>

<style>

    .upper-bar {

        position: absolute;

        top: 8px;
        left: 73px;
        right: 58px;

        height: 42px;
    }

    .content-renderer {
        position: absolute;

        left: 73px;
        top: 56px;
        bottom: 8px;
        right: 8px;

        background: transparent;
        border: transparent;

        overflow: scroll;
        overflow-x: hidden;

    }

    nav {
        position: absolute;

        left: 8px;
        top: 8px;
        bottom: 8px;

        width: 55px;

        border-radius: 16px;
    }

    .display {
        position: absolute;

        width: 306px;

        right: 0px;
        top: 0px;
        bottom: 0px;
    }

    .controlls {
        position: absolute;

        left: 0px;
        right: 0px;

        bottom: 0px;

        height: 60px;
        width: 100%;
    }

    .app-navigator {
        position: absolute;

        left: 0px;
        right: 331px;
        top: 0px;
        bottom: 0px;

        border-radius: 25px;
    }

    .app-content {
        position: absolute;

        left: 0px;
        right: 0px;
        top: 0px;
        bottom: 85px;
    }

    .content {
        position: absolute;

        left: 25px;
        right: 25px;
        top: 25px;
        bottom: 25px;

        opacity: 1;
    }
</style>
