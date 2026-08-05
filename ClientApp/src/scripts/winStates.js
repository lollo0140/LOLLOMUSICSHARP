import { ESend } from "./electronInvoker";
import gsap from "gsap";

let fullscreen = false;

let canAnimate = true;

export async function WinStateToOpen() {
    if (canAnimate) {
        canAnimate = false;
        await gsap.to(".MainTag", WINTOOPEN);
    }
}

export async function WinStateToPill() {
    if (canAnimate) {
        canAnimate = false;
        await gsap.to(".MainTag", WINTOPILL);
    }
}


export async function WinStateToFullscreen(state) {

    if (state) {
        await gsap.to(".MainTag", {
            left: 0,
            top: 0,
            bottom: 0,
            right: 0,
            height: "100%",
            borderRadius: 0,
            duration: 0.3,
            ease: "back.out(1)",
            border: "none"
        });
        fullscreen = true;

    } else {
        await gsap.to(".MainTag", {
            left: 100,
            right: 100,
            top: 20,
            border: "solid rgba(255, 255, 255, 0.3) 1px",
            borderRadius: 40,
            height: " calc(100% - 198px - 15px)",
            duration: 0.4,
            ease: "back.out(1)",
            opacity: 1,
            onComplete: () => {
                canAnimate = true;
            },
        });

        fullscreen = false;
    }

    canAnimate = true;

}

export const WINTOOPEN = {
    opacity: 0,
    duration: 0.2,
    onComplete: async () => {
        await ESend("setWinState", "open");
        await setTimeout(async () => {
            await gsap.to(".MainTag", {
                left: 100,
                right: 100,
                top: 20,
                bottom: "unset",
                border: "solid rgba(255, 255, 255, 0.3) 1px",
                borderRadius: 40,
                height: " calc(100% - 198px - 15px)",
                duration: 0.4,
                ease: "back.out(1)",
                opacity: 1,
                onComplete: () => {
                    canAnimate = true;
                },
            });
        }, 50);
    },
}

export const WINTOPILL = {
    left: "calc(50% - 300px / 2)",
    right: "calc(50% - 300px / 2)",
    top: 0,
    height: 52,
    border: "solid rgba(255, 255, 255, 0.3) 1px",
    duration: 0.4,
    opacity: 0,
    ease: "back.out(0.2)",
    onComplete: async () => {
        await ESend("setWinState", "close");
        await gsap.to(".MainTag", {
            opacity: 1,
            duration: 0.2,
        });
        canAnimate = true;
    },
}
