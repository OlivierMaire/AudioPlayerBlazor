

export var player = {
  get audio() { return document.getElementById("audio-player-blazor-html5"); },
  get progressbar() { return document.getElementById("audio-player-blazor-progressbar"); },
  get seekbars() { return document.querySelectorAll(".progress-seeking"); },
  get progressLists() { return document.querySelectorAll(".progress-list"); },
  get volumeSliders() { return document.getElementById("audio-player-blazor-volume") },
  progressListener: undefined,
  loadedPosition: 0,
  waveRender: true,
  waveRenderListener: function (val) { },
  registerWaveRenderListener: function (externalListenerFunction) {
    this.waveRenderListener = externalListenerFunction;
  },
  loaded: false,
  setDotNetRef(pDotNetReference) {
    if (!this.dotNetReference) {
      this.dotNetReference = pDotNetReference;
    }
  },
  dotNetReference: undefined,
  init() {
    if (!this.progressListener) {
      this.progressListener = this.audio.addEventListener("timeupdate", handleProgressEvent);
      this.progressbar.addEventListener('click', handleProgressClick);
      this.progressbar.addEventListener('mousemove', handleSeekMove);
      this.progressbar.addEventListener('mouseout', handleSeekMouseout);
      this.volumeSliders.querySelector('.vertical-progress-handle').addEventListener('click', handleVolumeClick);
      this.audio.addEventListener('volumechange', handleVolumeChange);
    }
  },
  load(src, mime, position) {
    console.log("load js audio");
    let audioSource = this.audio.querySelector("source");
    if (audioSource != null) {
      audioSource.src = src;
      audioSource.type = mime;
      this.audio.load();
      this.goToTimestamp(position);
      this.loadedPosition = position;
      this.init();
      this.loaded = true;
      handleVolumeChange();
      let canvas = document.getElementById("waveform");
      this.canvasCtx = canvas.getContext("2d");
      this.waveformInit();
    }
  },
  initMediaSession(title, artist, cover){
    if ('mediaSession' in navigator) {

      navigator.mediaSession.metadata = new MediaMetadata({
      title: title,
      artist: artist,
      artwork: [
          { src: cover},
      ]
      });
  
      navigator.mediaSession.setActionHandler('play', player.play );
      navigator.mediaSession.setActionHandler('pause', player.pause );
      // navigator.mediaSession.setActionHandler('seekbackward', function() {});
      // navigator.mediaSession.setActionHandler('seekforward', function() {});
      // navigator.mediaSession.setActionHandler('previoustrack', function() {});
      // navigator.mediaSession.setActionHandler('nexttrack', function() {});
  }
  },
  play() {
    player.audio.play();
  },
  pause() {
    player.audio.pause();
  },
  goToTimestamp(time) {
    this.audio.currentTime = time;
  },
  getTimestamp() {
    return this.audio.currentTime;
  },
  setVolume(value) {
    if (value > 1) value = 1;
    if (value < 0) value = 0;
    this.audio.volume = value;
  },
  stop() {

    this.audio.pause();
    this.waveformInitDone = false;
  },
  SetWaveRendering(shouldRender) {
    this.waveRender = shouldRender;
    this.waveRenderListener(shouldRender);
  },
  waveformInitDone: false,
  drawVisual: {},
  dataArray: [],
  bufferLength: 0,
  barWidth: 0,

  waveFormObj: {},
  canvasContext: {},
  waveformInit() {
    if (this.waveformInitDone) {
      return;
    }

    const audioCtx = new AudioContext();
    const analyser = audioCtx.createAnalyser();


    const distortion = audioCtx.createWaveShaper();
    const gainNode = audioCtx.createGain();
    const biquadFilter = audioCtx.createBiquadFilter();
    const convolver = audioCtx.createConvolver();


    analyser.minDecibels = -90;
    analyser.maxDecibels = -10;
    analyser.smoothingTimeConstant = 0.85;

    var source;
    source = audioCtx.createMediaElementSource(player.audio);

    let canvas = document.getElementById("waveform");
    this.canvasCtx = canvas.getContext("2d");
    canvas.width = canvas.clientWidth - 6;
    canvas.height = canvas.clientHeight - 6;
    const WIDTH = canvas.width;
    const HEIGHT = canvas.height;


    this.waveformInitDone = true;

    // var bufferLength
    // var dataArray
    const disconnectWaveForm = () => {
      analyser.disconnect(audioCtx.destination);
      source.disconnect(analyser);
    }

    const connectWaveForm = () => {
      source.connect(analyser);
      analyser.connect(audioCtx.destination);

      analyser.fftSize = 256;

      player.canvasCtx.clearRect(0, 0, WIDTH, HEIGHT);

      this.bufferLength = analyser.frequencyBinCount;

      this.dataArray = new Uint8Array(this.bufferLength);

      this.barWidth = (WIDTH / this.bufferLength);

      drawWave();
    }


    function drawWave() {
      {
        if (!player.waveRender) {
          player.canvasCtx.clearRect(0, 0, WIDTH, HEIGHT);
          return;
        }

        const width25p = player.bufferLength / 10;
        const opacityRate = 1 / width25p;

        player.drawVisual = requestAnimationFrame(drawWave);
        if (!player.dataArray || player.dataArray.length == 0) {
          cancelAnimationFrame(player.drawVisual);
          return;
        }
        analyser.getByteTimeDomainData(player.dataArray);
        player.canvasCtx.clearRect(0, 0, WIDTH, HEIGHT);

        let x = 0;
        for (let i = 0; i < player.bufferLength; i++) {
          const barHeight = player.dataArray[i] - 128; // base value is 128
          let opacity = 1;
          if (i < width25p) opacity = i * opacityRate;
          if (i > player.bufferLength - width25p) opacity = (player.bufferLength - i) * opacityRate;

          player.canvasCtx.fillStyle = `hsla(${barHeight * 2 + 120},100%,50%,${opacity})`; // change color hue based on value (120 is green)
          player.canvasCtx.fillRect(
            x,
            HEIGHT / 2 - barHeight,
            player.barWidth,
            barHeight
          );
          x += player.barWidth;
        }
      }
    }

    function onPlay() {
      connectWaveForm();
    }

    function onPause() {
      cancelAnimationFrame(player.drawVisual);
      player.canvasCtx.clearRect(0, 0, WIDTH, HEIGHT);
      disconnectWaveForm();
    }


    this.audio.addEventListener('play', onPlay);
    this.audio.addEventListener('pause', onPause);
    this.registerWaveRenderListener((val) => { if (!this.waveformInitDone) return; if (val) this.waveFormObj.draw(this); });


    this.waveFormObj = {
      draw: drawWave
    };

  }
}




function handleProgressEvent(event) {
  //console.log(event.target.currentTime);
  // console.log(player.audio.buffered);

  if (event.target.currentTime === 0) {
    player.goToTimestamp(player.loadedPosition);
  }
  else {
    player.dotNetReference.invokeMethodAsync('UpdateCurrentTimestampFromJs', event.target.currentTime);
  }
}

function handleProgressClick(event) {
  var xPos = (event.clientX - event.currentTarget.getBoundingClientRect().x) / event.currentTarget.offsetWidth;
  player.goToTimestamp(player.audio.duration * xPos);
}


function handleSeekMove(event) {
  // var xPos = event.layerX / event.currentTarget.offsetWidth;
  // player.progressLists[0].querySelector(".progress-seeking").style = `transform: scaleX(${xPos})`;

  let p = {
    x: event.clientX,
    y: event.clientY
  };

  let before = false;
  for (let i = player.progressLists.length - 1; i >= 0; i--) {
    let s = player.progressLists[i];
    let bb = s.getBoundingClientRect();
    if (before) {
      s.querySelector(".progress-seeking").style = `transform: scaleX(1)`;
    }
    else if (Math.floor(bb.left) <= p.x && p.x <= Math.floor(bb.right) + 2 && bb.top <= p.y && p.y <= bb.bottom) {
      // found selected item
      var xPos = (p.x - bb.left) / bb.width;
      if (xPos < 0) xPos = 0;
      if (xPos > .95) xPos = 1;
      // if (xPos == 1 && )
      // if (!event.target.className.includes("progress")) xPos = 0;

      s.querySelector(".progress-seeking").style = `transform: scaleX(${xPos})`;
      before = true;
    }
    else {
      s.querySelector(".progress-seeking").style = `transform: scaleX(0)`;
    }
  }

  // player.progressLists.findLast(s => {

  // })



}


function handleSeekMouseout(event) {
  // player.seekbars[0].value = 0;
  player.progressLists.forEach(p => p.querySelector(".progress-seeking").style = `transform: scaleX(0)`);
}


function handleVolumeClick(event) {
  var vol = 1 - (event.clientY - event.currentTarget.getBoundingClientRect().y) / event.currentTarget.offsetHeight;
  player.setVolume(vol);

}

function handleVolumeChange() {
  document.querySelector(".volume-slider").style = "display: block;";
  let cHeight = player.volumeSliders.querySelector('.vertical-progress-background').clientHeight;
  document.querySelector(".volume-slider").style = "";

  player.volumeSliders.querySelector('.vertical-progress-value').style = `height: calc(${cHeight}px * ${player.audio.volume}`;
  player.dotNetReference.invokeMethodAsync('UpdateVolumeFromJs', player.audio.volume);
}
