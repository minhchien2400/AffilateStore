export const fetchData = async (url) => {
  const response = await fetch(url);
  const data = await response.json();
  return data;
};

export const fetchDataBody = async (url, dataSend, method) => {
  const response = await fetch(url, {
    method: method,
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(dataSend),
  });
  const dataResponse = response.json();
  return dataResponse;
};
