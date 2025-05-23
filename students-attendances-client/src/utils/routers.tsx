import { createBrowserRouter, redirect } from 'react-router-dom';

import LoginPage from '@pages/LoginPage';
import { useUserStore } from '@core/store/store';
import CoursesListPage from '@pages/CoursesListPage';
import CourseInfoPage from '@pages/CourseInfoPage';
import LessonsListPage from '@pages/LessonsListPage';
import CreateLessonPage from '@pages/CreateLessonPage';
import ProcessingWaitPage from '@pages/ProcessingWaitPage';
import LessonAttendancePage from '@pages/LessonAttendancePage';
import { appStorage } from './localstorage';
import { roleKey } from './api';
import AdminPage from '@pages/admin/AdminPage';
import UpdateLessonPage from '@pages/UpdateLessonPage';
import NotFoundPage from '@pages/NotFoundPage';

export const loginPath = '/'
export const adminPath = '/admin'

const adminLoader = () => {
	const role = appStorage.getItem(roleKey)?.toLocaleLowerCase()
	return role !== 'admin' ? redirect(loginPath) : null
}

const protectedLoader = () => {
	const role = appStorage.getItem(roleKey)?.toLocaleLowerCase()
	if (role == 'admin') {
		return redirect(adminPath)
	}
	return !useUserStore.getState().user ? redirect(loginPath) : null
}
const publicLoader = () => {
	const role = appStorage.getItem(roleKey)?.toLocaleLowerCase()
	if (role == 'admin') {
		redirect(adminPath)
	}
	return useUserStore.getState().user ? redirect('/dashboard') : null
}

export const routers = createBrowserRouter([
	{
		path: '*',
		element: <NotFoundPage/>,
	},
	{
		path: '/',
		element: <LoginPage/>,
		loader: publicLoader
	},
	{
		path: '/dashboard',
		element: <CoursesListPage/>,
		loader: protectedLoader
	},
	{
		path: '/course/:id',
		element: <CourseInfoPage/>,
		loader: protectedLoader
	},
	{
		path: '/lessons',
		element: <LessonsListPage/>,
		loader: protectedLoader
	},
	{
		path: '/lessons/create',
		element: <CreateLessonPage/>,
		loader: protectedLoader
	},
	{
		path: '/lessons/edit/:id',
		element: <UpdateLessonPage/>,
		loader: protectedLoader
	},
	{
		path: '/lessons/attendance/:id',
		element: <LessonAttendancePage/>,
		loader: protectedLoader
	},
	{
		path: '/processing/:uuid',
		element: <ProcessingWaitPage/>,
		loader: protectedLoader
	},
	{
		path: '/processing/:uuid/:back',
		element: <ProcessingWaitPage/>,
		loader: protectedLoader
	},
	{
		path: adminPath,
		element: <AdminPage/>,
		loader: adminLoader
	}
])