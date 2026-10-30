import { Routes, Route } from 'react-router-dom';
import Profile from '../pages/Profile';
import Feed from '../pages/Feed';
import EditPost from '../pages/EditPost';
import CreatePost from '../pages/CreatePost';
import Followers from '../pages/Followers';

export default function AppRouter() {
  return (
    <Routes>
      <Route path="/" element={<Feed />} />
      <Route path="/profile/:id" element={<Profile />} />
      <Route path="/post/:id/edit" element={<EditPost />} />
      <Route path="/post/create" element={<CreatePost />} />
      <Route path="/followers/:id" element={<Followers />} />
    </Routes>
  );
}