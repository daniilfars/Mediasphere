import { Link, useParams } from 'react-router-dom';
import { userAPI } from '../api/userAPI';
import { followAPI } from '../api/followAPI';
import { useState } from 'react';
import { useEffect } from 'react';
import './Profile.css';

export default function Profile() {
  const [user, setUser] = useState(null);
  const [followers, setFollowers] = useState(null);
  const [followings, setFollowings] = useState(null);

  const [isUserLoading, setIsUserLoading] = useState(true);
  const [isFollowersLoading, setIsFollowersLoading] = useState(true);
  const [isFollowingsLoading, setIsFollowingsLoading] = useState(true);
  const [followError, setIsfollowError] = useState(false);

  const { id } = useParams();

  useEffect(() => {
    const loadProfile = async () => {
      try {
        const userData = await userAPI.getById(id);
        setUser(userData);

        if (userData && userData.userId) {
          try {
            const followersData = await followAPI.getFollowers(userData.userId);
            setFollowers(followersData);
          } catch {
            setIsfollowError(true);
          } finally {
            setIsFollowersLoading(false);
          }

          try {
            const followingsData = await followAPI.getFollowings(userData.userId);
            setFollowings(followingsData);
          } catch {
            setIsfollowError(true);
          } finally {
            setIsFollowingsLoading(false);
          }
        } else {
          setIsFollowersLoading(false);
          setIsFollowingsLoading(false);
        }
      }
      catch {}
      finally {
        setIsUserLoading(false);
      }
    }
    loadProfile();
  }, [id]);

  if(isUserLoading) return <div className="container">Загрузка профиля</div>;
  if(!user) return <div className="container">Пользователь не найден</div>;

  return (
    <header className="container profile-header">
      <div className="container profile-header-inner">
        <div className="profile-header-item">
          <h1 className="profile-username">{user.userName}</h1>
        </div>
        <div className="profile-header-item">
          {isFollowersLoading && isFollowingsLoading ? (
            <span>Загрузка</span>
          ) : followError ? (
            <span style={{ color: 'red' }}>Сервис подписок недоступен</span>
          ) : (
            <ul className="profile-header-list">
              <li className="profile-header-list-item">
                <Link to={`/followers/${id}`} className="profile-header-list-link">
                  <span>{followers.totalCount}</span> подписчиков
                </Link>
              </li>
              <li className="profile-header-list-item">
                <Link to={`/followings/${id}`} className="profile-header-list-link">
                  <span>{followings.totalCount}</span> подписок
                </Link>
              </li>
            </ul>
          )}
        </div>
      </div>
    </header>
  );
}