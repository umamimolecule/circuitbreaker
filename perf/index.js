import http from 'k6/http';
import { clearCircuitBreaker, endpoints, getUrl } from './api.js';
import { generateRandomKeys, getRandomItem } from './utils.js';

const keyCount = __ENV.KEY_COUNT || 1000;

export const options = {
  scenarios: {
    loadTest: {
      executor: 'constant-arrival-rate',
      duration: '1m',
      rate: __ENV.RATE || 2000,
      timeUnit: '1m',
      maxVUs: 2000,
      preAllocatedVUs: 2000,
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
