

export var player = {
  audio: document.getElementById("audio-player-blazor-html5"),
  progressListener: undefined,
  setDotNetRef(pDotNetReference){
    this.dotNetReference = pDotNetReference
  },
  dotNetReference: undefined,
  init() {
    if (!this.progressListener) {
      // this.jsObjectReference = DotNet.createJSObjectReference(window);
      this.progressListener = this.audio.addEventListener("timeupdate", handleProgressEvent);
    }
  },
  load(src, mime) {
    this.init();
    let audioSource = this.audio.querySelector("source");
    if (audioSource != null) {
      audioSource.src = src;
      audioSource.type = mime;
      this.audio.load();
    }
  },
  play() {
    this.audio.play();
  },
  pause() {
    this.audio.pause();
  },
  goToTimestamp(time) {
    this.audio.currentTime = time;
  },
  getTimestamp() {
    return this.audio.currentTime;
  },


}

function handleProgressEvent(event) {
  // console.log(event.target.currentTime);
  player.dotNetReference.invokeMethodAsync('UpdateCurrentTimestampFromJs', event.target.currentTime);

}



// DotNet.disposeJSObjectReference(jsObjectReference);