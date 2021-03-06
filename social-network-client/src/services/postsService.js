import { Post, Get } from './httpService';

const socialUrl = process.env.REACT_APP_SOCIAL_URL + 'posts';

export async function createPost(post) {
  const json = JSON.stringify(post);

  return await Post(socialUrl, json, true);
}

export async function getPost(postId) {
  return await Get(`${socialUrl}/${postId}`, true);
}
export async function getPosts(start, count) {
  return await Get(`${socialUrl}/${start}/${count}`, true);
}
