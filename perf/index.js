import http from 'k6/http';
import { clearCircuitBreaker, endpoints, getUrl } from './api.js';
import { generateRandomKeys, getRandomItem } from './utils.js';

const keyCount = 1000;

export const options = {
  scenarios: {
    loadTest: {
      executor: 'constant-arrival-rate',
      duration: '1s',
      rate: 1000,
      timeUnit: '1m',
      maxVUs: 1000,
      preAllocatedVUs: 1000,
    },
  },
};

export function setup() {
  clearCircuitBreaker();
  const keys = generateRandomKeys(keyCount);
  return {
    keys,
  };
}

export function teardown() {
  clearCircuitBreaker();
}

export default function (data) {
  const { keys } = data;
  const key = getRandomItem(keys);
  const endpoint = getRandomItem(endpoints);
  http.request(endpoint.method, getUrl(endpoint.path.replace('{key}', key)));
}
