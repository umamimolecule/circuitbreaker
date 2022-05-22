function generateRandomKeys(count) {
  const keys = [];
  for (let i = 0; i < count; i++) {
    keys[i] = uuidv4();
  }
  return keys;
}

function getRandomItem(arr) {
  return arr[Math.floor(arr.length * Math.random())];
}

function uuidv4() {
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
    let r = (Math.random() * 16) | 0,
      v = c === 'x' ? r : (r & 0x3) | 0x8;
    return v.toString(16);
  });
}

function isEnvVarTrue(name) {
  const value = __ENV[name];
  if (!value) {
    return false;
  }

  if (value.toString().toLowerCase() === 'false') {
    return false;
  }

  return true;
}

module.exports = {
  generateRandomKeys,
  getRandomItem,
  isEnvVarTrue,
  uuidv4,
};
